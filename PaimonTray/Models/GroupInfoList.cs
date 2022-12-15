using System.Collections.Generic;

namespace PaimonTray.Models
{
    /// <summary>
    /// The group info list model.
    /// </summary>
    public class GroupInfoList : List<object>
    {
        #region Constructors

        /// <summary>
        /// Initialise a group info list.
        /// </summary>
        /// <param name="items">The items.</param>
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        } // end constructor GroupInfoList

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The group info list key.
        /// </summary>
        public string Key { get; init; }

        #endregion Properties
    } // end class GroupInfoList
} // end namespace PaimonTray.Models