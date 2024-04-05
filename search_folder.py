import os
import re
from openpyxl import Workbook

entries = [
    (
        os.path.abspath("../String-Finder-1.0.0/IREM-Parts"),
        ["Page Template Library/IREM/Parts/.cs", 
        [r'SqlDataAdapter\("', r'SqlCommand\("', r'"UPDATE', r'"update', r'"SELECT', r'"select', r'"DELETE', r'"delete', r'"INSERT', r'"insert'],
        "IREM-Parts-c#-files.xlsx"],
        "csharp"
    ),
    (
        os.path.abspath("../String-Finder-1.0.0/IREM-Parts"),
        ["Page Template Library/IREM/Parts/.ascx", 
        [r"<asp:SqlDataSource"],
        "IREM-Parts-ascx-files.xlsx"],
        "dotnet"
    )
    ]
    
def save_to_wb(directory: str, excel_props: tuple, extension: str) -> None:
    """
    Saves an array of SQL injections received 
    from helper functions to an excel sheet
    
    args:
        string Directory: The absolute path of the folder you want to search through
        tuple excel_props: A tuple of 
            A string indicating what the folder is for/its path
            An array of patterns you want to search for in the folder
            The name of the excel sheet you create
        string extension: Which extension you want to look for

    Raises: Exception if the SQL source code list is empty
    """
    match extension: 
        case "csharp":
            sql_src = handle_csharp(directory, excel_props[1])
        case "dotnet":
            sql_src = handle_dot_net(directory, excel_props[1])
    # Don't create excel sheet if no patterns match 
    if len(sql_src) == 0:
        print(os.listdir(directory))
        return
        # If needed we can swap return for an Exception but it can be problematic
        raise Exception("Your excel sheet will be null. Terminating...")
    wb = Workbook()
    ws = wb.active
    ws.append(["Path: ", excel_props[0]])
    ws.append(["Pattern: ", str(excel_props[1])])
    ws.append([""])
    ws.append(["File", "Line Number", "Select Command"])
    for file, line_number, command in sql_src:
        ws.append([file, line_number, command])
    wb.save(excel_props[2])
    print(f"Excel file '{excel_props[2]}' has been created.")
    return

# If we want to exclude SqlCommand("") that aren't 
# a stored procedure we can update the pattern
def handle_csharp(directory, patterns):
    # Searches C# files in your directory
    sql_to_excel = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs") and not file.endswith(".designer.cs"):
                handle_os_walk(patterns, root, file, sql_to_excel)      
    return sql_to_excel

def handle_dot_net(directory, patterns):
    # Searches for ASP.NET (.ascx) files in your directory
    sql_to_excel = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".ascx"):
                handle_os_walk(patterns, root, file, sql_to_excel)      
    return sql_to_excel

def handle_os_walk(patterns, root, file, sql_to_excel):
    # Searches your files for the specified SQL injection pattern
    filepath = os.path.join(root, file)
    # you will get charmap codec can't decode byte XXXX errors if encoding not set properly
    with open(filepath, encoding="utf8") as f:
        content = f.readlines()
        for line_number, line in enumerate(content, 1):
            for pattern in patterns:
                # Trying to exclude commented out lines (lines that start with /)
                if re.match(r'^/', line.strip()): 
                    continue
                if re.search(pattern, line):
                    sql_to_excel.append(
                        (file, line_number, line.strip())
                    )
                    # Break to prevent duplicate entries if diff patterns observed on same line
                    break
    return sql_to_excel

# Start script and spread entry properties in save_to_wb function
for entry in entries:
    save_to_wb(*entry)
