import os
import re

from openpyxl import Workbook

directory = os.path.abspath("./String-Finder-1.0.0/Parts-X")

# Looking through C# files that DON'T have .designer in the extension
def find_sql_data_sources(directory):
    sql_data_sources = []
    # Depending on what SQL injections we actually want to grab, edit patterns
    # Possible patterns to include:
    # ONLY include SELECT, select, UPDATE, DELETE, etc if it is preceeded by new SqlCommand(" ??
    # Add DELETE, other SQL query modifiers 
    # patterns = [r'"SELECT ', r'"select ', r'SqlCommand\("', r'SqlDataAdapter\("']
    # patterns = [r'SqlCommand\("', r'SqlDataAdapter\("', r'SqlCommand\("', r'"UPDATE ', r'"update ', r'"SELECT ', r'"select ', r'"DELETE ', r'"delete ', r'"INSERT ', r'"insert ',]

    # Patterns: Most important is where we actually declare the full command 
    # (ex. SqlCommand\("', r'SqlDataAdapter\("', r'SqlCommand\("
    # Do we just want that part or do we want the extra injections that can me
    # referenced as variables such as declaring
    # a primitive like string sql1 = "SELECT DATA from EXAMPLE"
    
    patterns = [r'SqlCommand\("', r'SqlDataAdapter\("', r'SqlCommand\("']
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs") and not file.endswith(".designer.cs"):
                filepath = os.path.join(root, file)
                with open(filepath, "r") as f:
                    content = f.readlines()
                    for line_number, line in enumerate(content, 1):
                        if (line_number > 265):
                            for pattern in patterns:
                                if re.search(pattern, line):
                                    # Trying to ? exclude lines that contain //, we don't need to include commented lines
                                    match = re.match(r'^/', line.strip())
                                    if not match:
                                        sql_data_sources.append(
                                            (file, line_number, line.strip())
                                        )
                                        # If multiple patterns appear on same line, we will get duplicate entries (using break to prevent this)
                                        break

    return sql_data_sources


sql_data_sources = find_sql_data_sources(directory)
print(os.listdir(directory))
# Convert to excel file etc
wb = Workbook()
ws = wb.active
ws.append(["Path: IREM/Parts/.cs"])
ws.append(["Pattern: [r'SELECT ', r'SqlCommand\("', rSqlDataAdapter\("]'])
ws.append([""])
ws.append(["File", "Line Number", "Select Command"])

for file, line_number, command in sql_data_sources:
    ws.append([file, line_number, command])

excel_file = "c#-files.xlsx"
wb.save(excel_file)
print(f"Excel file '{excel_file}' has been created.")


#  Looking through ONLY .ascx files
def find_sql_data_sources2(directory):
    sql_data_sources2 = []
    patterns = patterns = [r"<asp:SqlDataSource"]
#   print(os.listdir(directory))
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".ascx"):
                filepath = os.path.join(root, file)
                with open(filepath, encoding="utf8") as f:
                    content = f.readlines()
                    for line_number, line in enumerate(content, 1):
                        for pattern in patterns:
                            if re.search(pattern, line):
                                sql_data_sources2.append(
                                    (file, line_number, line.strip())
                                )

    return sql_data_sources2


sql_data_sources2 = find_sql_data_sources2(directory)


# Convert to excel file etc
wb = Workbook()
ws = wb.active
ws.append(["Path: IREM/Parts/.ascx"])
ws.append(["Pattern: [r'<asp:SqlDataSource']"])
ws.append([""])
ws.append(["File", "Line Number", "Select Command"])

for file, line_number, command in sql_data_sources2:
    ws.append([file, line_number, command])

excel_file = "ascx-files.xlsx"
wb.save(excel_file)
print(f"Excel file '{excel_file}' has been created.")
