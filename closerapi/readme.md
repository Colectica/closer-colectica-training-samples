This is a simple access that shows how to access the Colectica Repository REST API from C# code.
The basic steps are:

1. Auto-generate a client for the REST API. This happens in the `closerapi.csproj`:

```
    <OpenApiReference Include="swagger.json" SourceUrl="https://closer.sandbox.colectica.org/swagger/v1/swagger.json" />
```

2. Request an API token, and set that token on an HTTP client for use in future calls.
3. Make the calls.
