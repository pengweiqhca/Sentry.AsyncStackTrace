> Use Ben.Demystifier extension which improves the readability of async stack traces.

## How To Use

``` C#
SentryOptions o = .....;

o.UseAsyncStackTraceFactory();
```

### Asp.net Core can use this way 

``` C#
services.AddTransient<ISentryStackTraceFactory, AsyncStackTraceFactory>();
```

![AsyncStackTrace compare](https://user-images.githubusercontent.com/3982826/46778807-edc0ae80-cd47-11e8-976c-ccb096cd1bdd.png)
