﻿namespace SignalRCharServerExample.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<Client> Clients { get; } = new();
    }
}
