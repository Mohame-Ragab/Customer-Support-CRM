import HomeIcon from '@mui/icons-material/Home'
import GroupIcon from '@mui/icons-material/Group'
import SecurityIcon from '@mui/icons-material/Security'
import HistoryIcon from '@mui/icons-material/History'
import SettingsApplicationsIcon from '@mui/icons-material/SettingsApplications'
import PaletteIcon from '@mui/icons-material/Palette'
import PeopleAltIcon from '@mui/icons-material/PeopleAlt'
import ChatIcon from '@mui/icons-material/Chat'
import DashboardIcon from '@mui/icons-material/Dashboard'
import ReplyIcon from '@mui/icons-material/Reply'
import MenuBookIcon from '@mui/icons-material/MenuBook'
import ConfirmationNumberIcon from '@mui/icons-material/ConfirmationNumber'
import AssessmentIcon from '@mui/icons-material/Assessment'
import Drawer from '@mui/material/Drawer'
import List from '@mui/material/List'
import ListItemButton from '@mui/material/ListItemButton'
import ListItemIcon from '@mui/material/ListItemIcon'
import ListItemText from '@mui/material/ListItemText'
import Toolbar from '@mui/material/Toolbar'
import { useTheme } from '@mui/material/styles'
import type { ComponentType } from 'react'
import { useTranslation } from 'react-i18next'
import { NavLink } from 'react-router-dom'

import { useAuth } from '@/features/auth/hooks/useAuth'
import { hasAnyRole } from '@/features/auth/authorization'
import { Roles } from '@/features/auth/roles'

import { DRAWER_WIDTH } from './layoutConstants'

interface NavItem {
  labelKey: string
  path: string
  icon: ComponentType
  /** Omit for items visible to every authenticated user; any-of when provided. */
  requiredRoles?: string[]
}

const STAFF_ROLES = [Roles.Admin, Roles.Supervisor, Roles.Manager, Roles.Agent]

/**
 * Navigation entries live in one array so adding a feature's nav item later
 * is a one-line change here, not new markup.
 */
const navItems: NavItem[] = [
  { labelKey: 'nav.home', path: '/', icon: HomeIcon },
  {
    labelKey: 'nav.dashboard',
    path: '/dashboard',
    icon: DashboardIcon,
    requiredRoles: STAFF_ROLES,
  },
  {
    labelKey: 'nav.customers',
    path: '/customers',
    icon: PeopleAltIcon,
    requiredRoles: STAFF_ROLES,
  },
  { labelKey: 'nav.chatQueue', path: '/agent/chats', icon: ChatIcon, requiredRoles: STAFF_ROLES },
  {
    labelKey: 'nav.managementDashboard',
    path: '/reports/dashboard',
    icon: AssessmentIcon,
    // Matches ReportsController's [Authorize(Roles = Admin,Supervisor)] - no Manager/Agent.
    requiredRoles: [Roles.Admin, Roles.Supervisor],
  },
  {
    labelKey: 'reports.agentPerformance.title',
    path: '/reports/agent-performance',
    icon: AssessmentIcon,
    requiredRoles: [Roles.Admin, Roles.Supervisor],
  },
  {
    labelKey: 'nav.knowledgeBase',
    path: '/knowledge-base',
    icon: MenuBookIcon,
    // Matches KnowledgeBaseContentController's [Authorize(Roles = Agent,Supervisor,Admin)] - no Manager.
    requiredRoles: [Roles.Admin, Roles.Supervisor, Roles.Agent],
  },
  {
    labelKey: 'nav.quickReplies',
    path: '/quick-replies',
    icon: ReplyIcon,
    requiredRoles: STAFF_ROLES,
  },
  { labelKey: 'nav.liveChat', path: '/chat', icon: ChatIcon, requiredRoles: [Roles.Customer] },
  {
    labelKey: 'nav.myTickets',
    path: '/portal/tickets',
    icon: ConfirmationNumberIcon,
    requiredRoles: [Roles.Customer],
  },
  {
    labelKey: 'nav.faqs',
    path: '/portal/faqs',
    icon: MenuBookIcon,
    requiredRoles: [Roles.Customer],
  },
  { labelKey: 'nav.users', path: '/users', icon: GroupIcon, requiredRoles: [Roles.Admin] },
  {
    labelKey: 'nav.rolePermissions',
    path: '/admin/role-permissions',
    icon: SecurityIcon,
    requiredRoles: [Roles.Admin],
  },
  {
    labelKey: 'nav.auditLogs',
    path: '/admin/audit-logs',
    icon: HistoryIcon,
    requiredRoles: [Roles.Admin],
  },
  {
    labelKey: 'nav.systemConfiguration',
    path: '/admin/system-configuration',
    icon: SettingsApplicationsIcon,
    requiredRoles: [Roles.Admin],
  },
  {
    labelKey: 'nav.branding',
    path: '/admin/branding',
    icon: PaletteIcon,
    requiredRoles: [Roles.Admin],
  },
]

interface SidebarProps {
  variant: 'permanent' | 'temporary'
  open: boolean
  onClose: () => void
}

/**
 * Dual-mode drawer: `permanent` on desktop (>= md, see AppLayout), `temporary`
 * (overlay + backdrop) below that. Anchors on the side MUI's theme.direction
 * calls "start" - left in LTR, right in RTL - so it flips automatically when
 * the user switches language (see hooks/useDirection.ts).
 */
export function Sidebar({ variant, open, onClose }: SidebarProps) {
  const { t } = useTranslation()
  const { user } = useAuth()
  const theme = useTheme()

  const anchor = theme.direction === 'rtl' ? 'right' : 'left'

  const visibleItems = navItems.filter(
    (item) => !item.requiredRoles || hasAnyRole(user, item.requiredRoles),
  )

  const handleItemClick = () => {
    if (variant === 'temporary') {
      onClose()
    }
  }

  return (
    <Drawer
      variant={variant}
      anchor={anchor}
      open={open}
      onClose={onClose}
      ModalProps={variant === 'temporary' ? { keepMounted: true } : undefined}
      sx={{
        width: DRAWER_WIDTH,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: DRAWER_WIDTH, boxSizing: 'border-box' },
      }}
    >
      {variant === 'permanent' && <Toolbar />}
      <List>
        {visibleItems.map(({ labelKey, path, icon: Icon }) => (
          <ListItemButton key={path} component={NavLink} to={path} end onClick={handleItemClick}>
            <ListItemIcon>
              <Icon />
            </ListItemIcon>
            <ListItemText primary={t(labelKey)} slotProps={{ primary: { noWrap: true } }} />
          </ListItemButton>
        ))}
      </List>
    </Drawer>
  )
}
