﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationToSellThings.APIs.Models;
using Microsoft.AspNetCore.Identity;

namespace ApplicationToSellThings.APIs.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationToSellThingsAPIsUser class
public class ApplicationToSellThingsAPIsUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    //public virtual ICollection<AddressModel> Addresses { get; set; }
}

