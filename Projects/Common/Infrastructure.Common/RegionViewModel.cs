namespace Infrastructure.Common
{
    public abstract class RegionViewModel :
            BaseViewModel,
            IViewPart
    {
        protected RegionViewModel()
        {
        }

        //public abstract void Dispose();
        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }
    }
}