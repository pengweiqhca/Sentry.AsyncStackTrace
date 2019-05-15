using System;
using System.Diagnostics;
using System.Reflection;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace Sentry.AsyncStackTrace
{
    public class AsyncStackTraceFactory : SentryStackTraceFactory
    {
        public AsyncStackTraceFactory(SentryOptions options) : base(options) { }

        protected override StackTrace CreateStackTrace(Exception exception) =>
            exception == null
                ? EnhancedStackTrace.Current()
                : new EnhancedStackTrace(exception);

        protected override SentryStackFrame CreateFrame(StackFrame stackFrame, bool isCurrentStackTrace) =>
            InternalCreateFrame(stackFrame, false);

        protected override MethodBase GetMethod(StackFrame stackFrame) =>
            stackFrame is EnhancedStackFrame esf ? esf.MethodInfo.MethodBase : base.GetMethod(stackFrame);
    }
}
