﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Networking;

namespace I2.Loc
{
    using TranslationDictionary = Dictionary<string, TranslationQuery>;

    public class TranslationJob_GET : TranslationJob_WWW
    {
        TranslationDictionary _requests;
        Action<TranslationDictionary, string> _OnTranslationReady;
        List<string> mQueries;
        public string mErrorMessage;

        public TranslationJob_GET(TranslationDictionary requests, Action<TranslationDictionary, string> OnTranslationReady)
        {
            _requests = requests;
            _OnTranslationReady = OnTranslationReady;

            mQueries = GoogleTranslation.ConvertTranslationRequest(requests, true);

            GetState();
        }

        void ExecuteNextQuery()
        {
            if (mQueries.Count == 0)
            {
                mJobState = eJobState.Succeeded;
                return;
            }

            int lastQuery = mQueries.Count - 1;
            var query = mQueries[lastQuery];
            mQueries.RemoveAt(lastQuery);

            string url = string.Format("{0}?action=Translate&list={1}", LocalizationManager.GetWebServiceURL(), query);
            www = new UnityWebRequest(url);
        }

        public override eJobState GetState()
        {
            if (www != null && www.isDone)
            {
                ProcessResult(www.downloadHandler.data, www.error);
                www.Dispose();
                www = null;
            }

            if (www==null)
            {
                ExecuteNextQuery();
            }

            return mJobState;
        }

        public void ProcessResult(byte[] bytes, string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                var wwwText = Encoding.UTF8.GetString(bytes, 0, bytes.Length); //www.text
                errorMsg = GoogleTranslation.ParseTranslationResult(wwwText, _requests);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    if (_OnTranslationReady!=null)
                        _OnTranslationReady(_requests, null);
                    return;
                }
            }

            mJobState = eJobState.Failed;
            mErrorMessage = errorMsg;
        }
    }
}