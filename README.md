> Use Ben.Demystifier extension which improves the readability of async stack traces.

## How To Use

``` C#
SentryOptions o = .....;

o.AsyncStackTraceFactory();
```

### Asp.net Core can use this way 

``` C#
services.AddTransient<ISentryStackTraceFactory, AsyncStackTraceFactory>();
```

