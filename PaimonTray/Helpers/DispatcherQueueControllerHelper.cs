using Serilog;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.System;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The dispatcher queue controller helper.
    /// </summary>
    internal class DispatcherQueueControllerHelper
    {
        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~DispatcherQueueControllerHelper()
        {
            _dispatcherQueueController = null;
        } // end destructor

        #endregion Destructor

        #region Fields

        /// <summary>
        /// The dispatcher queue controller.
        /// </summary>
        private object _dispatcherQueueController;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Create a dispatcher queue controller on the caller's thread.
        /// Reference: https://docs.microsoft.com/en-us/windows/win32/api/dispatcherqueue/nf-dispatcherqueue-createdispatcherqueuecontroller
        /// </summary>
        /// <param name="options">The threading affinity and type of COM apartment for the created dispatcher queue controller.</param>
        /// <param name="dispatcherQueueController">The created dispatcher queue controller.</param>
        /// <returns>S_OK for success; otherwise a failure code.</returns>
        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)]
            ref object dispatcherQueueController);

        /// <summary>
        /// Ensure the dispatcher queue controller.
        /// </summary>
        /// <returns>A flag indicating if the dispatcher queue controller is ensured.</returns>
        public bool EnsureDispatcherQueueController()
        {
            if (DispatcherQueue.GetForCurrentThread() is not null) return true;

            if (_dispatcherQueueController is not null) return true;

            DispatcherQueueOptions options;
            options.apartmentType =
                2; // Reference: https://docs.microsoft.com/en-us/windows/win32/api/dispatcherqueue/ne-dispatcherqueue-dispatcherqueue_thread_apartmenttype
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType =
                2; // Reference: https://docs.microsoft.com/en-us/windows/win32/api/dispatcherqueue/ne-dispatcherqueue-dispatcherqueue_thread_type

            var hResult =
                CreateDispatcherQueueController(options,
                    ref _dispatcherQueueController); // Reference: https://docs.microsoft.com/en-us/windows/win32/seccrypto/common-hresult-values

            if (hResult is 0) return true;

            Log.Warning($"Failed to ensure the dispatcher queue controller (HRESULT: {hResult}).");
            return false;
        } // end method EnsureDispatcherQueueController

        #endregion Methods

        #region Structures

        /// <summary>
        /// The dispatcher queue options structure.
        /// Reference: https://docs.microsoft.com/en-us/windows/win32/api/dispatcherqueue/ns-dispatcherqueue-dispatcherqueueoptions
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct DispatcherQueueOptions : IEquatable<DispatcherQueueOptions>
        {
            #region Fields

            /// <summary>
            /// The size of this dispatcher queue options structure.
            /// </summary>
            internal int dwSize;

            /// <summary>
            /// The thread affinity for the created dispatcher queue controller.
            /// </summary>
            internal int threadType;

            /// <summary>
            /// Specify whether to initialise COM apartment on the new thread as an application single-threaded apartment (ASTA) or single-threaded apartment (STA).
            /// </summary>
            internal int apartmentType;

            #endregion Fields

            #region Methods

            /// <summary>
            /// Determine whether 2 values are equal.
            /// </summary>
            /// <param name="left">The value for comparison on the left of the operator.</param>
            /// <param name="right">The value for comparison on the right of the operator.</param>
            /// <returns>A flag indicating whether 2 values are equal.</returns>
            public static bool operator ==(DispatcherQueueOptions left, DispatcherQueueOptions right)
            {
                return left.Equals(right);
            } // end operator ==

            /// <summary>
            /// Determine whether 2 values are not equal.
            /// </summary>
            /// <param name="left">The value for comparison on the left of the operator.</param>
            /// <param name="right">The value for comparison on the right of the operator.</param>
            /// <returns>A flag indicating whether 2 values are not equal.</returns>
            public static bool operator !=(DispatcherQueueOptions left, DispatcherQueueOptions right)
            {
                return !(left == right);
            } // end operator !=

            /// <summary>
            /// Determine whether 2 structures are equal.
            /// </summary>
            /// <param name="other">The structure to compare with the current structure.</param>
            /// <returns>A flag indicating whether 2 structures are equal.</returns>
            public bool Equals(DispatcherQueueOptions other)
            {
                return apartmentType == other.apartmentType && dwSize == other.dwSize && threadType == other.threadType;
            } // end method Equals(DispatcherQueueOptions)

            /// <summary>
            /// Determine whether 2 object instances are equal.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns>A flag indicating whether 2 object instances are equal.</returns>
            public override bool Equals(object obj)
            {
                return obj is DispatcherQueueOptions equatable && Equals(equatable);
            } // end method Equals(Object)

            /// <summary>
            /// Get the hash code.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                return Package.Current.Id.FamilyName.GetHashCode();
            } // end method GetHashCode

            #endregion Methods
        } // end struct DispatcherQueueOptions

        #endregion Structures
    } // end class DispatcherQueueControllerHelper
} // end namespace PaimonTray.Helpers