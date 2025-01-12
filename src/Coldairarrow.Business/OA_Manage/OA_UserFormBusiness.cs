﻿using Coldairarrow.Entity.OA_Manage;
using Coldairarrow.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coldairarrow.Business.Base_Manage;
using System.Collections.Concurrent;
using System.Threading;
using LinqKit;
using AutoMapper;
using EFCore.Sharding;
using AutoMapper.QueryableExtensions;

namespace Coldairarrow.Business.OA_Manage
{
    public class OA_UserFormBusiness : BaseBusiness<OA_UserForm>, IOA_UserFormBusiness, ITransientDependency
    {
        readonly IMapper _mapper;
        readonly IBase_UserBusiness _userBus;
        public OA_UserFormBusiness(IBase_UserBusiness userBus, IDbAccessor db, IMapper mapper)
            : base(db)
        {
            _mapper = mapper;
            _userBus = userBus;
        }

        private static ConcurrentBag<string> _queues = new ConcurrentBag<string>();

        public Task QueueWork(string id)
        {
            _queues.Add(id);
            return Task.CompletedTask;
        }

        public async Task<string> DequeueWork(string id)
        {
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                if (_queues.Contains(id))
                {
                    if (_queues.TryTake(out id))//3m超时
                        return id;
                }
            }

            return null;
        }



        #region 外部接口

        public async Task<PageResult<OA_UserFormDTO>> GetDataListAsync(PageInput<OA_UserFormInputDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<OA_UserForm>();

            //筛选
            if (!input.Search.condition.IsNullOrEmpty() && !input.Search.keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<OA_UserForm, bool>(
                    ParsingConfig.Default, false, $@"{input.Search.condition}.Contains(@0)", input.Search.keyword);
                where = where.And(newWhere);
            }

            //按字典筛选
            if (input.SearchKeyValues != null)
            {
                foreach (var keyValuePair in input.SearchKeyValues)
                {
                    var newWhere = DynamicExpressionParser.ParseLambda<OA_UserForm, bool>(
                        ParsingConfig.Default, false, $@"{keyValuePair.Key}.Contains(@0)", keyValuePair.Value);
                    where = where.And(newWhere);
                }
            }

            if (!input.Search.userId.IsNullOrEmpty())
            {
                where = where.And(p => p.UserIds.Contains("^" + input.Search.userId + "^") && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.applicantUserId.IsNullOrEmpty())
            {
                where = where.And(p => p.ApplicantUserId == input.Search.applicantUserId && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.alreadyUserIds.IsNullOrEmpty())
            {
                where = where.And(p => p.AlreadyUserIds.Contains("^" + input.Search.alreadyUserIds + "^"));
            }

            if (!input.Search.creatorId.IsNullOrEmpty())
            {
                where = where.And(p => p.CreatorId == input.Search.creatorId);
            }


            return await q.Where(where).ProjectTo<OA_UserFormDTO>(_mapper.ConfigurationProvider).GetPageResultAsync(input);
        }

    

        public int GetDataListCount(List<string> jsonids, OAStatus status)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<OA_UserForm>();

            if (!jsonids.IsNullOrEmpty())
            {
                where = where.And(p => jsonids.Contains(p.DefFormJsonId));
            }

            if ((int)status >= 0)
            {
                where = where.And(p => p.Status == (int)status);
            }

            return q.Where(where).Count();
        }


        public async Task<OA_UserFormDTO> GetTheDataAsync(string id)
        {
            //return Mapper.Map<OA_UserFormDTO>(await GetEntityAsync(id));

            var form = await (from a in Db.GetIQueryable<OA_UserForm>()
                              let j1 = Db.GetIQueryable<OA_UserFormStep>(false).Where(b => a.Id == b.UserFormId).OrderBy(b => b.CreateTime).ToList()                             
                              where a.Id.Equals(id)
                              select new
                              {
                                  UserForm = a,
                                  Comments = j1
                              }).FirstOrDefaultAsync();

            OA_UserFormDTO formdto = _mapper.Map<OA_UserFormDTO>(form.UserForm);
            formdto.Comments = _mapper.Map<List<OA_UserFormStepDTO>>(form.Comments);

            foreach (var comment in formdto.Comments)
            {
                comment.Avatar = await _userBus.GetAvatar(comment.CreatorId);
            }

            return formdto;

        }

        public async Task AddDataAsync(OA_UserForm data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(OA_UserForm data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        #endregion


        #region 历史数据查询
        public async Task<int> GetHistoryDataCountAsync(Input<OA_UserFormInputDTO> input)
        {
            var where = LinqHelper.True<OA_UserForm>();

            //筛选
            if (!input.Search.condition.IsNullOrEmpty() && !input.Search.keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<OA_UserForm, bool>(
                    ParsingConfig.Default, false, $@"{input.Search.condition}.Contains(@0)", input.Search.keyword);
                where = where.And(newWhere);
            }

            if (!input.Search.userId.IsNullOrEmpty())
            {
                where = where.And(p => p.UserIds.Contains("^" + input.Search.userId + "^") && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.applicantUserId.IsNullOrEmpty())
            {
                where = where.And(p => p.ApplicantUserId == input.Search.applicantUserId && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.alreadyUserIds.IsNullOrEmpty())
            {
                where = where.And(p => p.AlreadyUserIds.Contains("^" + input.Search.alreadyUserIds + "^"));
            }

            if (!input.Search.creatorId.IsNullOrEmpty())
            {
                where = where.And(p => p.CreatorId == input.Search.creatorId);
            }

            var count = await GetHistoryDataCount(where, input.Search.start, input.Search.end, "CreateTime");

            return count;
        }
        public async Task<List<OA_UserFormDTO>> GetHistoryDataListAsync(Input<OA_UserFormInputDTO> input)
        {
            var where = LinqHelper.True<OA_UserForm>();

            //筛选
            if (!input.Search.condition.IsNullOrEmpty() && !input.Search.keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<OA_UserForm, bool>(
                    ParsingConfig.Default, false, $@"{input.Search.condition}.Contains(@0)", input.Search.keyword);
                where = where.And(newWhere);
            }

            if (!input.Search.userId.IsNullOrEmpty())
            {
                where = where.And(p => p.UserIds.Contains("^" + input.Search.userId + "^") && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.applicantUserId.IsNullOrEmpty())
            {
                where = where.And(p => p.ApplicantUserId == input.Search.applicantUserId && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.alreadyUserIds.IsNullOrEmpty())
            {
                where = where.And(p => p.AlreadyUserIds.Contains("^" + input.Search.alreadyUserIds + "^"));
            }

            if (!input.Search.creatorId.IsNullOrEmpty())
            {
                where = where.And(p => p.CreatorId == input.Search.creatorId);
            }

            var dataList = await GetHistoryDataQueryable(where, input.Search.start, input.Search.end, "CreateTime").ProjectTo<OA_UserFormDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return dataList;
        }

        [Transactional]
        public async Task<PageResult<OA_UserFormDTO>> GetPageHistoryDataListAsync(PageInput<OA_UserFormInputDTO> input)
        {   
            var where = LinqHelper.True<OA_UserForm>();

            //筛选
            if (!input.Search.condition.IsNullOrEmpty() && !input.Search.keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<OA_UserForm, bool>(
                    ParsingConfig.Default, false, $@"{input.Search.condition}.Contains(@0)", input.Search.keyword);
                where = where.And(newWhere);
            }

            if (!input.Search.userId.IsNullOrEmpty())
            {
                where = where.And(p => p.UserIds.Contains("^" + input.Search.userId + "^") && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.applicantUserId.IsNullOrEmpty())
            {
                where = where.And(p => p.ApplicantUserId == input.Search.applicantUserId && p.Status == (int)OAStatus.Being);
            }

            if (!input.Search.alreadyUserIds.IsNullOrEmpty())
            {
                where = where.And(p => p.AlreadyUserIds.Contains("^" + input.Search.alreadyUserIds + "^"));
            }

            if (!input.Search.creatorId.IsNullOrEmpty())
            {
                where = where.And(p => p.CreatorId == input.Search.creatorId);
            }

            var dataList = await GetHistoryDataQueryable(where, input.Search.start, input.Search.end, "CreateTime").ProjectTo<OA_UserFormDTO>(_mapper.ConfigurationProvider).GetPageResultAsync(input);
            dataList.Data.ForEach(async p =>
            {
                p.Avatar = await _userBus.GetAvatar(p.CreatorId);
            });

            return dataList;
        }

        #endregion
    }


}