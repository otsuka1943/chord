﻿using dp2weixin.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dp2weixinWeb.ApiControllers
{
    public class WxUserController : ApiController
    {
        private WxUserDatabase wxUserDb = WxUserDatabase.Current;

        // 获取全部绑定账户，包括读者与工作人员
        [HttpGet]
        public WxUserResult Get()
        {
            WxUserResult result = new WxUserResult();
            List<WxUserItem> list = wxUserDb.GetUsers();//"*", 0, -1).Result;
            result.users = list;
            return result;
        }

        public WxUserResult Get(string weixinId)
        {          
            if (weixinId == "recover")
            {
                return dp2WeiXinService.Instance.RecoverUsers();
            }
            else
            {
                WxUserResult result = new WxUserResult();
                List<WxUserItem> list= wxUserDb.GetAllByWeixinId(weixinId);
                result.users = list;
                return result;
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResult ResetPassword(string libId,
            string name, 
            string tel)
        {
            ApiResult result = new ApiResult();

            string strError = "";
            int nRet = dp2WeiXinService.Instance.ResetPassword(libId,
                name,
                tel,
                out strError);
            result.errorCode = nRet;
            result.errorInfo = strError;

            return result;
        }

        [HttpPost]
        public ApiResult Setting(string weixinId, UserSettingItem item)
        {
            ApiResult result = new ApiResult();

            //string setting_lib = libId;

            try
            {
                UserSettingDb.Current.SetLib(item);
            }
            catch (Exception ex)
            {
                result.errorCode = -1;
                result.errorInfo = ex.Message;
                return result;
            }

            return result;        
        }


        // POST api/<controller>
        [HttpPost]
        public WxUserResult Bind(WxUserItem item)
        {
            // 返回对象
            WxUserResult result = new WxUserResult();
            //result.userItem = null;
           // result.apiResult = new ApiResult();

            // 前端有时传上来是这个值
            if (item.prefix == "null")
                item.prefix = "";
            WxUserItem userItem = null;
            string strError="";
            int nRet= dp2WeiXinService.Instance.Bind(item.libId,
                item.prefix,
                item.word,
                item.password,
                item.weixinId,
                out userItem,
                out strError);
            if (nRet == -1)
            {
                result.errorCode = -1;
                result.errorInfo = strError;
            }
            result.users = new List<WxUserItem>();
            result.users.Add(userItem);

            return result;// repo.Add(item);
        }



        // 修改密码
        [HttpPost]
        public ApiResult ChangePassword(string libId,
            string patron,
            string oldPassword,
            string newPassword)
        {
            ApiResult result = new ApiResult();

            string strError = "";
            int nRet = dp2WeiXinService.Instance.ChangePassword(libId,
                patron,
                oldPassword,
                newPassword,
                out strError);
            result.errorCode = nRet;
            result.errorInfo = strError;

            return result;
        }


        // PUT api/<controller>/5
        [HttpPut]
        public void ActivePatron(string weixinId,string id)
        {
            if (weixinId == "null")
                weixinId = "";

            if (id == "null")
                id = "";

            WxUserItem user = wxUserDb.GetById(id);
            if (user != null)
                dp2WeiXinService.Instance.SetActivePatron(user);
        }



        // DELETE api/<controller>/5
        [HttpDelete]
        public ApiResult Delete(string id)
        {
            ApiResult result = new ApiResult();
            string strError = "";
            int nRet = dp2WeiXinService.Instance.Unbind(id, out strError);
            if (nRet == -1)
            {
                result.errorCode = -1;
                result.errorInfo = strError;
            }

            return result;
        }



    }
}
