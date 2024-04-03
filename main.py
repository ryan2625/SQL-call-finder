import os
import re

from openpyxl import Workbook

directory = os.path.abspath("../python/String-Finder-1.0.0/Parts-X")

# Looking through C# files that DON'T have .designer in the extension
def find_sql_data_sources(directory):
    sql_data_sources = []
    patterns = [r"SELECT ", r'SqlCommand\("', r'SqlDataAdapter\("']

    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs") and not file.endswith(".designer.cs"):
                filepath = os.path.join(root, file)
                with open(filepath, "r") as f:
                    content = f.readlines()
                    for line_number, line in enumerate(content, 1):
                        for pattern in patterns:
                            if re.search(pattern, line):
                                # Exclude comments, don't need them
                                match = re.match(r'^/', line.strip())
                                if not match:
                                    sql_data_sources.append(
                                        (file, line_number, line.strip())
                                    )

    return sql_data_sources


sql_data_sources = find_sql_data_sources(directory)

# Convert to excel file etc
wb = Workbook()
ws = wb.active
ws.append(["File", "Line Number", "Select Command"])

for file, line_number, command in sql_data_sources:
    ws.append([file, line_number, command])

excel_file = "sql_select_commands.xlsx"
wb.save(excel_file)
print(f"Excel file '{excel_file}' has been created.")


# Search .ascx files
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
ws.append(["File", "Line Number", "Select Command"])

for file, line_number, command in sql_data_sources2:
    ws.append([file, line_number, command])

excel_file = "ascx_sql_select_commands.xlsx"
wb.save(excel_file)
print(f"Excel file '{excel_file}' has been created.")
