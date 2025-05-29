//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WebVella.BlazorTrace.Utility;
//public static class WvAsyncUtility
//{
//    private static readonly TaskFactory _taskFactory = new
//        TaskFactory(CancellationToken.None,
//                    TaskCreationOptions.None,
//                    TaskContinuationOptions.None,
//                    TaskScheduler.Default);

//    public static TResult RunSync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default(CancellationToken))
//        => _taskFactory
//            .StartNew(func)
//            .Unwrap()
//            .GetAwaiter()
//            .GetResult();

//    public static void RunSync(Func<Task> func, CancellationToken cancellationToken = default(CancellationToken))
//        => _taskFactory
//            .StartNew(func, cancellationToken)
//            .Unwrap()
//            .GetAwaiter()
//            .GetResult();
//}
