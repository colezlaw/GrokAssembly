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
    <companyName>OWASP Contributors</companyName>
    <productName>GrokAssembly</productName>
    <productVersion>3.0.0.0</productVersion>
    <comments>Inspects a .NET Assembly to determine Company, Product, and Version information</comments>
    <fileDescription>GrokAssembly</fileDescription>
    <fileName>/Users/jeremy/Projects/GrokAssembly/GrokAssembly/bin/Release/netcoreapp2.0/GrokAssembly.dll</fileName>
    <fileVersion>3.0.0.0</fileVersion>
    <internalName>GrokAssembly.exe</internalName>
    <originalFilename>GrokAssembly.exe</originalFilename>
    <fullName>GrokAssembly, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null</fullName>
    <namespaces>
        <namespace>GrokAssembly</namespace>
    </namespaces>
</assembly>
```

[dependencycheck]: https://github.com/jeremylong/DependencyCheck
