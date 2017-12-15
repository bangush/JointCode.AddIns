﻿//Copyright (c) 2012 Tomaz Koritnik

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
//modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace JointCode.AddIns.Core.Storage.Transactions
{
    // <源码增加>
    class SegmentComparer : IComparer<Segment>
    {
        internal static SegmentComparer Instance = new SegmentComparer();
        public int Compare(Segment x, Segment y)
        {
            return x.Location.CompareTo(y.Location);
        }
    }
    // </源码增加>

    /// <summary>
    /// Segment holds information about area that have already been backed up or doesn't require backup
    /// </summary>
    internal class Segment
    {
        #region Public fields
        /// <summary>
        /// Segment start location in bytes
        /// </summary>
        public long Location;
        /// <summary>
        /// Segment size in bytes
        /// </summary>
        public long Size;
        #endregion

        #region Construction
        /// <summary>
        /// COnstruct a segment
        /// </summary>
        public Segment(long location, long size)
        {
            this.Location = location;
            this.Size = size;
        }
        #endregion
    }
}
