import { Component, type ErrorInfo, type ReactNode } from 'react'

import { ErrorState } from '@/components/ui/ErrorState'

interface ErrorBoundaryProps {
  children: ReactNode
}

interface ErrorBoundaryState {
  hasError: boolean
}

/**
 * Catches rendering errors anywhere below it in the tree so a bug in one part
 * of the UI doesn't blank the whole app. React error boundaries must be class
 * components - there is no hook equivalent. Logs to the console only (see
 * docs/frontend-architecture.md, "Logging") - never logs error.message/stack
 * if it might contain user-entered data beyond what React already reports.
 */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  override state: ErrorBoundaryState = { hasError: false }

  static getDerivedStateFromError(): ErrorBoundaryState {
    return { hasError: true }
  }

  override componentDidCatch(error: Error, info: ErrorInfo): void {
    if (import.meta.env.DEV) {
      console.error('Unhandled error caught by ErrorBoundary:', error, info.componentStack)
    }
  }

  override render(): ReactNode {
    if (this.state.hasError) {
      return <ErrorState />
    }
    return this.props.children
  }
}
