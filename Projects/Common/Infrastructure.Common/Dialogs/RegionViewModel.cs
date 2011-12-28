namespace Infrastructure.Common
{
    public abstract class RegionViewModel :
            BaseViewModel,
            IViewPart
    {
        protected RegionViewModel()
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }
    }
}