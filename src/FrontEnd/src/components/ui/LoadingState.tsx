import Box from '@mui/material/Box'
import CircularProgress from '@mui/material/CircularProgress'

interface LoadingStateProps {
  /** Accessible label for screen readers; visually hidden by CircularProgress itself. */
  label?: string
}

/** Generic loading placeholder. No feature-specific variant belongs here. */
export function LoadingState({ label = 'Loading' }: LoadingStateProps) {
  return (
    <Box
      role="status"
      aria-label={label}
      sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', p: 4 }}
    >
      <CircularProgress />
    </Box>
  )
}
