﻿using Debby.Admin.Core.Extensions;
using Debby.Admin.Core.Model;
using Debby.Admin.Core.Model.Interfaces;
using Debby.Admin.Core.ModelConnectors.Interfaces;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Debby.Admin.Core.ModelConnectors
{
    public class EFModelConnector<TContext> : IModelConnector where TContext : DbContext
    {
        private TContext context;
        private Microsoft.Data.Entity.Metadata.IModel model;

        public EFModelConnector(TContext context)
        {
            this.context = context;
            this.model = context.Model;
        }

        public IEntityType GetEntityType(string entityName)
        {
            var entity = DebbyAdmin.Entities.FirstOrDefault(e => e.Name == entityName);
            if (entity == null)
                throw new ArgumentException(
                    String.Format("The provided Entity Name doesn't exist : {0}", entityName));

            return GetEntityType(entity);
        }

        public IEntityType GetEntityType(Type type)
        {
            var entityType = new EntityType(type);

            var efEntityType = model.GetEntityType(type);
            foreach (var prop in efEntityType.Properties)
            {
                var property = new Property(entityType, prop.Name, prop.PropertyType);
                property.IsNullable = prop.IsNullable;
                property.IsReadOnly = prop.IsReadOnly;
                entityType.AddProperty(property);
            }

            return entityType;
        }

        public async Task<IList<dynamic>> RetrieveRecords<T>() where T : class
        {
            var dbSet = context.Set<T>();
            var entities = await dbSet.ToListAsync();

            var data = new List<dynamic>();
            foreach (var entity in entities)
                data.Add(entity);

            return data;
        }

        public async Task<dynamic> AddRecord<T>(IDictionary<string, object> data) where T : class
        {
            T obj = data.FromDynamic<T>();

            await context.Set<T>().AddAsync(obj);

            var result = await context.SaveChangesAsync();

            return obj;
        }
    }
}