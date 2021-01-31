namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;
   using System.Threading;
   using System.Threading.Tasks;

   /// <summary>
   ///    Class EventUtils.
   /// </summary>
   public static class EventUtils
   {
      /// <summary>
      ///    Delegate GenericDelegate
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="val">The value.</param>
      public delegate void GenericDelegate<in T>(T val);

      /// <summary>
      ///    Delegate GenericDelegateTask
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="val">The value.</param>
      /// <returns>Task.</returns>
      public delegate void GenericDelegateTask<in T>(T val);

      /// <summary>
      ///    Delegate NoParamsDelegate
      /// </summary>
      public delegate void NoParamsDelegate();

      /// <summary>
      ///    Delegate NoParamsDelegateTask
      /// </summary>
      /// <returns>Task.</returns>
      public delegate void NoParamsDelegateTask();

      /// <summary>
      ///    Raises the specified sender.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The e.</param>
      public static void Raise<T>(this EventHandler<T> handler,
                                  object sender,
                                  T e) where T : EventArgs
      {
         handler?.Invoke(sender, e);
      }

      /// <summary>
      ///    Raises the specified sender.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      public static void Raise(this EventHandler handler,
                               object sender,
                               EventArgs e)
      {
         handler?.Invoke(sender, e);
      }

      /// <summary>
      ///    Raises the on different thread.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The e.</param>
      public static void RaiseOnDifferentThread<T>(this EventHandler<T> handler,
                                                   object sender,
                                                   T e) where T : EventArgs
      {
         if (handler != null)
         {
            Task.Factory.StartNewOnDifferentThread(() => handler.Raise(sender, e));
         }
      }

      /// <summary>
      ///    Raises the on different thread.
      /// </summary>
      /// <param name="handler">The handler.</param>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      public static Task RaiseOnDifferentThread(this EventHandler handler,
                                                object sender,
                                                EventArgs e)
      {
         if (handler != null)
         {
            return Task.Factory.StartNewOnDifferentThread(() => handler.Raise(sender, e));
         }

         return Task.FromResult(false);
      }

      /// <summary>
      ///    Starts the new on different thread.
      /// </summary>
      /// <param name="taskFactory">The void factory.</param>
      /// <param name="action">The action.</param>
      /// <returns>Task.</returns>
      public static Task StartNewOnDifferentThread(this TaskFactory taskFactory,
                                                   Action action)
      {
         return taskFactory.StartNew(action, new CancellationToken());
      }
   }
}
