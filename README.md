> Use Ben.Demystifier extension which improves the readability of async stack traces.

## How To Use

### Asp.net Core

``` C#
services.AddTransient<ISentryStackTraceFactory, AsyncStackTraceFactory>();
```

### Other

``` C#
SentryOptions o = .....;

o.UseStackTraceFactory(new SentryStackTraceFactory(o));
```
