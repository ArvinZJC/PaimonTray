using System.Collections.Generic;

namespace PaimonTray.Models
{
    /// <summary>
    /// The group info list model.
    /// </summary>
    public class GroupInfoList : List<object>
    {
        #region Properties

        /// <summary>
        /// The key.
        /// </summary>
        public Dictionary<string, object> Key { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise a group info list.
        /// </summary>
        /// <param name="items">The items.</param>
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        } // end constructor GroupInfoList

        #endregion Constructors
    } // end class GroupInfoList
} // end namespace PaimonTray.Models