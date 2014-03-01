GrokAssembly
============

GrokAssembly is a simple .NET/mono project used for getting name and version
information out of an assembly. It is primarily used for the
[OWASP Dependency Check][dependencycheck] project to identify company, product,
and version information.

Usage:
------

```cmd
$ GrokAssembly <assembly>
```

or

```bash
$ mono GrokAssembly.exe <assembly>
```

[dependencycheck]: https://github.com/jeremylong/DependencyCheck
