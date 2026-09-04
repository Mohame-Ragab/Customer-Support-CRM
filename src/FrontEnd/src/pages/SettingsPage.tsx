import { Container, Typography, Card, CardContent, Stack } from '@mui/material'
import { ChangePasswordForm } from '@/features/auth/components/ChangePasswordForm'
import { ProfileForm } from '@/features/auth/components/ProfileForm'

export function SettingsPage() {
  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Settings
      </Typography>

      <Stack spacing={3} sx={{ mt: 3 }}>
        <Card>
          <CardContent>
            <ProfileForm />
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <ChangePasswordForm />
          </CardContent>
        </Card>
      </Stack>
    </Container>
  )
}
