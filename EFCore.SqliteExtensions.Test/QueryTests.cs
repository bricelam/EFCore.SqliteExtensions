﻿using System.Linq;
using Bricelam.EntityFrameworkCore.Sqlite.Test;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bricelam.EntityFrameworkCore.Sqlite
{
    public class QueryTests
    {
        [Fact]
        public void TimeSpan_TotalDays()
        {
            using (var db = new TestContext())
            {
                db.Database.OpenConnection();
                db.Database.EnsureCreated();

                var days = Enumerable.First(
                    from e in db.Entities
                    where e.Id == 1
                    select e.TimeSpan.TotalDays);

                // TODO: Assert SQL
                Assert.Equal(0.847314814815, days, precision: 12);
            }
        }
    }
}
