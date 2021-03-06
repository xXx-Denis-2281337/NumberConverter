﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NumberConverter.Models;

namespace NumberConverter
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NCService : INCService
    {

        public NCService()
        {
            this._context = new NCContext();
        }

        private NCContext _context { get; set; }

        public int ConvertNumber(int value, int userId)
        {
            Conversion conversion = new Conversion();
            conversion.Original = value.ToString();
            conversion.Converted = ToRoman(value);
            conversion.ConversionTime = DateTime.Now.ToString();
            conversion.UserId = userId;
            _context.Conversions.Add(conversion);
            _context.SaveChanges();
            return conversion.Id;
        }

        public string GenerateToken(User user)
        {
            user.CurrentToken = Guid.NewGuid().ToString();
            return user.CurrentToken;
        }

        public List<Conversion> GetHistory(int userId)
        {
            var result = _context.Conversions.Where(c => c.UserId == userId).ToList();
            return result;
        }

        public int GetUser(string name, string pass)
        {
            var user = _context.Users.SingleOrDefault(u => u.Name == name && u.Password == pass);
            if (user == null) return -1;
            user.CurrentToken = GenerateToken(user);
            _context.SaveChanges();
            return user.Id;
        }

        public int GetUserByToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.CurrentToken == token);
            if (user == null) return -1;
            user.CurrentToken = GenerateToken(user);
            _context.SaveChanges();
            return user.Id;
        }

        public User GetUserData(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            return user;
        }

        public int RegisterUser(string name, string pass)
        {
            User user = new User();
            user.Name = name;
            user.Password = pass;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Id;
        }

        //Helper method that converts numbers to roman
        public static string ToRoman(int number)
        {
            if (number < 0) throw new ArgumentOutOfRangeException("insert positive value");
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }

        public Conversion GetConversion(int convId)
        {
            var conversion = _context.Conversions.SingleOrDefault(c => c.Id == convId);
            return conversion;
        }
    }
}
