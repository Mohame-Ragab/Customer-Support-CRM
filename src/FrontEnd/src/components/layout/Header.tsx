import AppBar from '@mui/material/AppBar'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import IconButton from '@mui/material/IconButton'
import MenuIcon from '@mui/icons-material/Menu'
import Toolbar from '@mui/material/Toolbar'
import Typography from '@mui/material/Typography'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'

import { appConfig, type SupportedLanguage } from '@/config/appConfig'
import { useAuth } from '@/features/auth/hooks/useAuth'
import { useBranding } from '@/features/branding/hooks/useBranding'

interface HeaderProps {
  onMenuClick: () => void
  showMenuButton: boolean
}

/** App chrome: brand + language switcher + (when authenticated) account settings/logout. No CRM navigation lives here. */
export function Header({ onMenuClick, showMenuButton }: HeaderProps) {
  const { t, i18n } = useTranslation()
  const { status, logout } = useAuth()
  const { data: branding } = useBranding()

  const nextLanguage: SupportedLanguage = i18n.language === 'ar' ? 'en' : 'ar'

  return (
    <AppBar
      position="fixed"
      color="primary"
      enableColorOnDark
      sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}
    >
      <Toolbar sx={{ minWidth: 0 }}>
        {showMenuButton && (
          <IconButton
            color="inherit"
            aria-label="open navigation"
            edge="start"
            onClick={onMenuClick}
            sx={{ mr: 2, display: { xs: 'inline-flex', md: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
        )}

        {branding?.logoDataUrl && (
          <Box
            component="img"
            src={branding.logoDataUrl}
            alt={t('app.name', { defaultValue: appConfig.appName })}
            sx={{ height: 32, mr: 1 }}
          />
        )}

        <Typography variant="h6" component="h1" noWrap sx={{ flexGrow: 1, minWidth: 0 }}>
          {t('app.name', { defaultValue: appConfig.appName })}
        </Typography>

        <Button color="inherit" onClick={() => void i18n.changeLanguage(nextLanguage)}>
          {t('actions.switchLanguage')}
        </Button>

        {status === 'authenticated' && (
          <>
            <Button color="inherit" component={Link} to="/settings">
              {t('nav.settings')}
            </Button>

            <Button color="inherit" onClick={logout}>
              {t('actions.logout')}
            </Button>
          </>
        )}
      </Toolbar>
    </AppBar>
  )
}
