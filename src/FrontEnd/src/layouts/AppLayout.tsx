import { useState } from 'react'
import Box from '@mui/material/Box'
import Toolbar from '@mui/material/Toolbar'
import useMediaQuery from '@mui/material/useMediaQuery'
import { useTheme } from '@mui/material/styles'
import { Outlet } from 'react-router-dom'

import { Header } from '@/components/layout/Header'
import { Sidebar } from '@/components/layout/Sidebar'
import { DRAWER_WIDTH } from '@/components/layout/layoutConstants'

/**
 * Shell for authenticated routes: header + sidebar + routed content.
 * Responsive: permanent sidebar on desktop (>= md), collapsible temporary
 * drawer (opened via the Header's hamburger button) below that - see
 * docs/frontend-architecture.md, "Responsive layout".
 */
export function AppLayout() {
  const theme = useTheme()
  const isDesktop = useMediaQuery(theme.breakpoints.up('md'))
  const [mobileOpen, setMobileOpen] = useState(false)

  const handleDrawerToggle = () => setMobileOpen((prev) => !prev)
  const handleDrawerClose = () => setMobileOpen(false)

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', width: '100%', overflowX: 'hidden' }}>
      <Header onMenuClick={handleDrawerToggle} showMenuButton={!isDesktop} />
      <Sidebar
        variant={isDesktop ? 'permanent' : 'temporary'}
        open={isDesktop || mobileOpen}
        onClose={handleDrawerClose}
      />
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          width: { xs: '100%', md: `calc(100% - ${DRAWER_WIDTH}px)` },
          p: { xs: 2, sm: 3 },
        }}
      >
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  )
}
