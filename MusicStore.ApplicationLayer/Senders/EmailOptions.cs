﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.Senders
{
    public class EmailOptions
    {
        public string SendGridApiKey { get; set; }
        public string SendGridUser { get; set; }
    }
}
