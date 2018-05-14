// Copyright (c) Microsoft Corporation.  All rights reserved.

//Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;

namespace BreadcrumbBarDemo
{
    public class SampleData
    {
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public Brush Icon
        {
            get { return _brush; }
            set { _brush = value; }
        }

        public List<SampleData> Children
        {
            get { return _children; }
        }

        private string _title = String.Empty;
        private List<SampleData> _children = new List<SampleData>();
        private Brush _brush = Brushes.Transparent;
    }
}