using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace ConsoleDemo {
  class Program {
    static void Main(string[] args) {
      var q = new BlockingCollection<int>(new ConcurrentQueue<int>( ));

      var task = new Task(( ) => {
        for (int i = 0; i < 50; i++) {
          q.Add(i);
          Thread.Sleep(100);
        }
        q.CompleteAdding( );
      });
      task.Start( );

      // Step 1 - simple consume the enumerable
      //foreach (int i in q.GetConsumingEnumerable()) {
      //  Console.WriteLine(i);
      //}

      // Step 2 - consume from an observable - the standard scheduler 
      // blocks the current thread until the consumption is done
      var observableValues = q.GetConsumingEnumerable( ).ToObservable( );
      observableValues.Subscribe(v => Console.WriteLine(v));

      while (true) {
        Console.WriteLine("Main thread");
        Thread.Sleep(1000);
      }

      // Step 3 - using a different scheduler, the subscription runs in parallel 
      // to the main thread
      //var observableValues = q.GetConsumingEnumerable( ).ToObservable(TaskPoolScheduler.Default);
      //observableValues.Subscribe(v => Console.WriteLine("(Thread {0}) {1}", Thread.CurrentThread.ManagedThreadId, v));

      //while (true) {
      //  Console.WriteLine("(Thread {0}) Main thread", Thread.CurrentThread.ManagedThreadId);
      //  Thread.Sleep(1000);
      //}
    }
  }
}
