namespace MVsToolkit.Demo
{
    public interface IDemoInterface 
    {
        public void DemoMethod();
    }

    [System.Serializable]
    public struct InlineClass
    {
        public string name;
        public bool isValid;
    }
}