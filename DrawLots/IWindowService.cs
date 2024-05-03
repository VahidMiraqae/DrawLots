namespace DrawLots
{
    internal interface IWindowService
    {
        void Close(object vm, bool? result = null);
        bool? OpenDialog(object vm);
    }
}
