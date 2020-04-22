namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

   /// <summary>
   ///    Class ErrorUtils.
   /// </summary>
   public static class ErrorUtils
   {
      /// <summary>
      ///    Considers the argument error.
      /// </summary>
      /// <param name="condition">if set to <c>true</c> [condition].</param>
      /// <param name="message">The message.</param>
      public static void ConsiderArgumentError(bool condition, string message)
      {
         if (condition)
         {
            ThrowArgumentError(message);
         }
      }

      /// <summary>
      ///    Throws the argument error.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <exception cref="ArgumentException"></exception>
      public static void ThrowArgumentError(string message)
      {
         throw new ArgumentException(message);
      }
   }
}