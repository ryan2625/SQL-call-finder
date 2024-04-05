# SQL-injection-finder

This program will take any C# or ASP.NET files and record all instances of an SQL injection or use of an SQL controller in .NET. It will then export the results to an excel 
sheet with three columns that contain the file name, the line number, and the SQL injection.
<h1></h1>

If we use the pattern and entry below with the test file Sql_Command_Test.xlsx 
```python
entries = [
    (
        os.path.abspath("../String-Finder-1.0.0/Parts-Test"),
        ["Test/.cs", 
        [r'SqlDataAdapter\("', r'SqlCommand\("', r'"UPDATE', r'"update', r'"SELECT', r'"select', r'"DELETE', r'"delete', r'"INSERT', r'"insert', r'SELECT', r'UPDATE', r'DELETE', r'CREATE', r'WHERE', r'SqlCommand\('],
        "Sql_Command_Test.xlsx"],
        "csharp"
    ),
```

We will create the following excel sheet:

