﻿using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public class Base_TestBusiness : BaseBusiness<Base_Test>, IBase_TestBusiness, ITransientDependency
    {
        public Base_TestBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口

        public async Task<PageResult<Base_Test>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Base_Test>();
            var search = input.Search;

            //筛选
            if (!search.condition.IsNullOrEmpty() && !search.keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<Base_Test, bool>(
                    ParsingConfig.Default, false, $@"{search.condition}.Contains(@0)", search.keyword);
                where = where.And(newWhere);
            }

            //按字典筛选
            if (input.SearchKeyValues != null)
            {
                foreach (var keyValuePair in input.SearchKeyValues)
                {
                    var newWhere = DynamicExpressionParser.ParseLambda<Base_Test, bool>(
                        ParsingConfig.Default, false, $@"{keyValuePair.Key}.Contains(@0)", keyValuePair.Value);
                    where = where.And(newWhere);
                }
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Base_Test> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(Base_Test data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(Base_Test data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        #endregion

        #region 私有成员

        #endregion
    }
}