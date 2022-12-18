namespace UnityEngine.PostProcessing
{
    public sealed class MiniAAttribute : PropertyAttribute
    {
        public readonly float min;

        public MiniAAttribute(float min)
        {
            this.min = min;
        }
    }
}
