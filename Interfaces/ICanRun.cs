namespace Com.MarcusTS.SharedUtils.Interfaces
{
   using Com.MarcusTS.SharedUtils.Utils;

   public interface ICanRun
   {
      IThreadSafeAccessor IsRunning { get; }
   }
}