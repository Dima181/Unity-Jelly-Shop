namespace Mine.CodeBase.Framework.Manager.ResourceFactory
{
    public class ResourceFactoryBuilderBase<T>
        where T : ResourceFactoryBuilderBase<T>
    {
        #region Fields

        protected bool IsInject;
        protected bool IsAddressable;

        #endregion


#if VCONTAINER_SUPPORT
        public T ByInject
        {
            get
            {
                IsInject = true;

                return this as T;
            }
        }
#endif


#if ADDRESSABLE_SUPPORT
        public T ByAddressable
        {
            get
            {
                isAddressable = true;

                return this as T;
            }
        }
#endif
    }
}
