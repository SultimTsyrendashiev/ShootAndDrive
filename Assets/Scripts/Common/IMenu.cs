namespace SD
{
    interface IMenu
    {
        void Init(UI.MenuController menuController);
        /// <summary>
        /// Called when menu must be activated.
        /// NOTE: menu game object must be enabled manually in this method
        /// </summary>
        void Activate();
        /// <summary>
        /// Called when menu must be deactivated.
        /// NOTE: menu game object must be disabled manually in this method
        /// </summary>
        void Deactivate();
    }
}
