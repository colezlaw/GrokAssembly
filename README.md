[![Build Status](https://travis-ci.org/jeremylong/GrokAssembly.svg?branch=master)](https://travis-ci.org/jeremylong/GrokAssembly)
GrokAssembly
============

GrokAssembly is a simple .NET core project used for extracting extended properties
information, such as company, product name, and version, from an assembly. The tool
is primarily used within the [OWASP Dependency Check][dependencycheck] project to
identify Common Platform Identifiers (CPE) and report on known vulnerabilities.

Usage:
------

```bash
$ dotnet GrokAssembly.dll <assembly>
```

### Example Output
```bash
$ dotnet GrokAssembly.dll GrokAssembly.dll
```
```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly>
    <CompanyName>OWASP Contributors</CompanyName>
    <ProductName>GrokAssembly</ProductName>
    <ProductVersion>3.0.0.0</ProductVersion>
    <Comments>Inspects a .NET Assembly to determine Company, Product, and Version information</Comments>
    <FileDescription>GrokAssembly</FileDescription>
    <FileName>/Users/jeremy/Projects/GrokAssembly/GrokAssembly/bin/Release/netcoreapp2.0/GrokAssembly.dll</FileName>
    <FileVersion>3.0.0.0</FileVersion>
    <InternalName>GrokAssembly.exe</InternalName>
    <OriginalFilename>GrokAssembly.exe</OriginalFilename>
    <fullname>GrokAssembly, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null</fullname>
    <namespaces>
        <namespace>GrokAssembly</namespace>
    </namespaces>
</assembly>
```

[dependencycheck]: https://github.com/jeremylong/DependencyCheck
