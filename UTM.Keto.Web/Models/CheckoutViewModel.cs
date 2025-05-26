using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(100, ErrorMessage = "Shipping address cannot exceed 100 characters")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }
        
        [Required(ErrorMessage = "Shipping method is required")]
        [Display(Name = "Shipping Method")]
        public string ShippingMethod { get; set; }
        
        [Display(Name = "Full Name")]
        public string UserName { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
        
        public CartViewModel Cart { get; set; }
        
        public CheckoutViewModel()
        {
            Cart = new CartViewModel();
        }
    }
} 