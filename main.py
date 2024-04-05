import os
import re
from openpyxl import Workbook

entries = [
    (
        os.path.abspath("../python/String-Finder-1.0.0/Parts-X"),
        ["IREM/Parts/.cs", 
        [r'SqlDataAdapter\("', r'SqlCommand\("', r'"UPDATE', r'"update', r'"SELECT', r'"select', r'"DELETE', r'"delete', r'"INSERT', r'"insert'],
        "IREM-Parts-c#-files.xlsx"],
        "csharp"
    ),

    (
        os.path.abspath("../python/String-Finder-1.0.0/Parts-X"),
        ["IREM/Parts/.ascx", 
        [r"<asp:SqlDataSource"],
        "IREM-Parts-ascx-files.xlsx"],
        "dotnet"
    ),
    ]

def handle_os_walk(patterns, root, file, sql_to_excel):
    filepath = os.path.join(root, file)
    # If you don't set the encoding here, you will
    # keep getting charmap codec can't decode
    # byte XXXX errors
    with open(filepath, encoding="utf8") as f:
        content = f.readlines()
        for line_number, line in enumerate(content, 1):
                for pattern in patterns:
                    if re.search(pattern, line):
                        # Trying to exclude commented out lines (lines that start with /)
                        match = re.match(r'^/', line.strip())
                        if not match:
                            sql_to_excel.append(
                                (file, line_number, line.strip())
                            )
                            # Break to prevent duplicate entries if diff patterns observed on same line
                            break
    return sql_to_excel

def save_to_wb(directory, excel_props, extension):
    match extension: 
        case "csharp":
            sql_src = handle_csharp(directory, excel_props[1])
        case "dotnet":
            sql_src = handle_dot_net(directory, excel_props[1])
    # Don't create excel sheet if no patterns match 
    if len(sql_src) == 0:
        print(os.listdir(directory))
        raise Exception("Your excel sheet will be null. Terminating...")
    wb = Workbook()
    ws = wb.active
    ws.append(["Path: ", excel_props[0]])
    ws.append(["Pattern: ", str(excel_props[1])])
    ws.append([""])
    ws.append(["File", "Line Number", "Select Command"])
    for file, line_number, command in sql_src:
        ws.append([file, line_number, command])
    excel_file = excel_props[2]
    wb.save(excel_file)
    print(f"Excel file '{excel_file}' has been created.")
    return

def handle_csharp(directory, patterns):
    sql_to_excel = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs") and not file.endswith(".designer.cs"):
                handle_os_walk(patterns, root, file, sql_to_excel)      
    return sql_to_excel

def handle_dot_net(directory, patterns):
    sql_to_excel = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".ascx"):
                handle_os_walk(patterns, root, file, sql_to_excel)      
    return sql_to_excel

for entry in entries:
    save_to_wb(*entry)
