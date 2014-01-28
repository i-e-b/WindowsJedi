using System.Collections;

namespace WindowsJedi.WinApis
{
    /// <summary>
    /// A collection for Window objects.
    /// </summary>
    public class WindowCollection : CollectionBase {
        #region Public Properties
        /// <summary>
        /// Gets a window from teh collection.
        /// </summary>
        public Desktop.Window this[int index] {
            get {
                return (Desktop.Window)List[index];
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a window to the collection.
        /// </summary>
        /// <param name="wnd">Window to add.</param>
        public void Add (Desktop.Window wnd) {
            // adds a widow to the collection.
            List.Add(wnd);
        }
        #endregion
    }
}