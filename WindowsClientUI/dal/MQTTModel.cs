/**
 * Mobius Software LTD
 * Copyright 2015-2017, Mobius Software LTD
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
 
namespace com.mobius.software.windows.iotbroker.ui.win7.dal
{
    using SQLite.CodeFirst;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class MQTTModel : DbContext
    {
        // Your context has been configured to use a 'localModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'com.mobius.software.windows.iotbroker.ui.win7.localModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'localModel' 
        // connection string in the application configuration file.
        public MQTTModel() : base("name=MQTTModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Account> Accounts { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<MQTTModel>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);

            modelBuilder.Entity<Account>().HasKey(a => a.ID);
            modelBuilder.Entity<Account>().Property(a => a.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Account>().Property(a => a.Pass).HasMaxLength(100);
            modelBuilder.Entity<Account>().Property(a => a.UserName).HasMaxLength(100);
            modelBuilder.Entity<Account>().Property(a => a.ServerHost).HasMaxLength(100);

            modelBuilder.Entity<Topic>().HasKey(t => t.ID);
            modelBuilder.Entity<Topic>().Property(t => t.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Topic>().HasRequired(t => t.Account).WithMany().WillCascadeOnDelete();
            
            modelBuilder.Entity<Message>().HasKey(m => m.ID);
            modelBuilder.Entity<Message>().Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Message>().HasRequired(t => t.Account).WithMany().WillCascadeOnDelete();
        }
    }
}