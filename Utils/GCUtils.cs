namespace Com.MarcusTS.SharedUtils.Utils
{
   using System;

   /// <summary>
   ///    Class GCUtils.
   /// </summary>
   public static class GCUtils
   {
      /// <summary>
      ///    Triggers the gc.
      /// </summary>
      private static void TriggerGC()
      {
         Console.WriteLine("Starting GC.");

         GC.Collect();
         GC.WaitForPendingFinalizers();
         GC.Collect();

         Console.WriteLine("GC finished.");
      }
   }
}
