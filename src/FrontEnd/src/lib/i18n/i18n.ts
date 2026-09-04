import i18next from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import { initReactI18next } from 'react-i18next'

import { appConfig } from '@/config/appConfig'

import en from './resources/en/common.json'
import ar from './resources/ar/common.json'

/**
 * i18next setup. Only one namespace ("common") exists at this stage - feature
 * namespaces (e.g. "customers") are added alongside their feature, not here.
 * Initialized eagerly (not inside a React component) so a non-React module
 * like lib/api/apiClient.ts can read `i18n.language` synchronously.
 */
void i18next
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { common: en },
      ar: { common: ar },
    },
    ns: ['common'],
    defaultNS: 'common',
    fallbackLng: appConfig.defaultLanguage,
    supportedLngs: appConfig.supportedLanguages,
    interpolation: {
      escapeValue: false, // React already escapes output.
    },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage'],
    },
  })

export default i18next
