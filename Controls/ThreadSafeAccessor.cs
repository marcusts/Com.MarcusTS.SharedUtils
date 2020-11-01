namespace Com.MarcusTS.SharedUtils.Controls
{
   using System.Threading;

   /// <summary>
   ///    Interface IThreadSafeAccessor
   /// </summary>
   public interface IThreadSafeAccessor
   {
      /// <summary>
      ///    Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      object ReadStoredValue();

      /// <summary>
      ///    Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      void WriteStoredValue(object valueToStore);
   }

   /// <summary>
   ///    Class ThreadSafeAccessor.
   ///    Implements the <see cref="SharedUtils.Controls.IThreadSafeAccessor" />
   ///    Implements the <see cref="Com.MarcusTS.SharedUtils.Controls.IThreadSafeAccessor" />
   /// </summary>
   /// <seealso cref="Com.MarcusTS.SharedUtils.Controls.IThreadSafeAccessor" />
   /// <seealso cref="SharedUtils.Controls.IThreadSafeAccessor" />
   public class ThreadSafeAccessor : IThreadSafeAccessor
   {
      /// <summary>
      ///    The stored value
      /// </summary>
      private object _storedValue;

      /// <summary>
      ///    Initializes a new instance of the <see cref="ThreadSafeAccessor" /> class.
      /// </summary>
      /// <param name="storedValue">The stored value.</param>
      public ThreadSafeAccessor(object storedValue = null)
      {
         if (storedValue != null)
         {
            WriteStoredValue(storedValue);
         }
      }

      /// <summary>
      ///    Reads the stored value.
      /// </summary>
      /// <returns>System.Object.</returns>
      public object ReadStoredValue()
      {
         return Interlocked.CompareExchange(ref _storedValue, 0, 0);
      }

      /// <summary>
      ///    Writes the stored value.
      /// </summary>
      /// <param name="valueToStore">The value to store.</param>
      public void WriteStoredValue(object valueToStore)
      {
         Interlocked.Exchange(ref _storedValue, valueToStore);
      }
   }
}
