using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Arrowgene.O2Jam.Server.Common;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    // 密码通常不在Account对象中传来传去，以策安全
}