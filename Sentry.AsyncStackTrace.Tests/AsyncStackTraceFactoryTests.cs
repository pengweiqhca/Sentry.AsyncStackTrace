using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sentry;
using Sentry.AsyncStackTrace;
using Xunit;

// ReSharper disable once CheckNamespace
// Stack trace filters out Sentry frames by namespace
namespace Other.Tests
{
    public class AsyncStackTraceFactoryTests
    {
        private class Fixture
        {
            public SentryOptions SentryOptions { get; set; } = new SentryOptions();
            public AsyncStackTraceFactory GetSut() => new AsyncStackTraceFactory(SentryOptions);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void Create_NoExceptionAndDefaultAttachStackTraceOption_NullResult()
        {
            var sut = _fixture.GetSut();

            Assert.Null(sut.Create());
        }

        [Fact]
        public void Create_NoExceptionAndAttachStackTraceOptionOn_CurrentStackTrace()
        {
            _fixture.SentryOptions.AttachStacktrace = true;
            var sut = _fixture.GetSut();

            var stackTrace = sut.Create();

            Assert.NotNull(stackTrace);
            Assert.Equal(nameof(Create_NoExceptionAndAttachStackTraceOptionOn_CurrentStackTrace), stackTrace.Frames.Last().Function);
            Assert.DoesNotContain(stackTrace.Frames, p => p.Function == "CreateFrame");
        }

        [Fact]
        public void Create_WithExceptionAndDefaultAttachStackTraceOption_HasStackTrace()
        {
            var sut = _fixture.GetSut();

            Exception exception;
            try
            {
                Throw();
                void Throw() => throw null;
            }
            catch (Exception e) { exception = e; }

            Assert.NotNull(sut.Create(exception));
        }

        [Fact]
        public void Create_WithExceptionAndAttachStackTraceOptionOn_HasStackTrace()
        {
            _fixture.SentryOptions.AttachStacktrace = true;
            var sut = _fixture.GetSut();

            Exception exception;
            try
            {
                Throw();
                void Throw() => throw null;
            }
            catch (Exception e) { exception = e; }

            var stackTrace = sut.Create(exception);

            Assert.Equal(new StackTrace(exception, true).FrameCount, stackTrace.Frames.Count);
        }

        [Fact]
        public async Task Create_WithAsyncExceptionAndAttachStackTraceOptionOn_HasStackTraceAsync()
        {
            var sut = _fixture.GetSut();
            Exception exception;
            try
            {
                await new AsyncMethodClass().MethodAsync(1, 2, 3, 4);
                Assert.False(true);
                return;
            }
            catch (Exception e) { exception = e; }
            var stackTrace = sut.Create(exception);
            Assert.NotNull(stackTrace);
            Assert.Equal(new EnhancedStackTrace(exception).FrameCount, stackTrace.Frames.Count);
            Assert.NotEqual(new StackTrace(exception).FrameCount, stackTrace.Frames.Count);
        }

        [Fact]
        public async Task Create_WithAsyncExceptionFramesAsync()
        {
            var sut = _fixture.GetSut();

            Exception exception;
            try
            {
                await new AsyncMethodClass().MethodAsync(1, 2, 3, 4);
                Assert.False(true);
                return;
            }
            catch (Exception e) { exception = e; }

            if (Type.GetType("Mono.Runtime") != null) return; //Unsupport Mono

            foreach (var frame in sut.Create(exception).Frames)
            {
                Assert.DoesNotContain("MoveNext", frame.Function);
                Assert.DoesNotContain("GetResult", frame.Function);
            }
        }

        private class AsyncMethodClass
        {
            private Dictionary<int, int> _dict = new Dictionary<int, int>();
            public async Task<int> MethodAsync(int v0, int v1, int v2, int v3)
              => await MethodAsync(v0, v1, v2);
            private async Task<int> MethodAsync(int v0, int v1, int v2)
               => await MethodAsync(v0, v1);
            private async Task<int> MethodAsync(int v0, int v1)
               => await MethodAsync(v0);
            private async Task<int> MethodAsync(int v0)
               => await MethodAsync();
            private async Task<int> MethodAsync()
            {
                await Task.Delay(1000);
                int value = 0;
                foreach (var i in Sequence(0, 5))
                {
                    value += i;
                }
                return value;
            }
            private IEnumerable<int> Sequence(int start, int end)
            {
                for (var i = start; i <= end; i++)
                {
                    foreach (var item in Sequence(i))
                    {
                        yield return item;
                    }
                }
            }
            private IEnumerable<int> Sequence(int start)
            {
                var end = start + 10;
                for (var i = start; i <= end; i++)
                {
                    _dict[i] = _dict[i] + 1; // Throws exception
                    yield return i;
                }
            }
        }
    }
}
