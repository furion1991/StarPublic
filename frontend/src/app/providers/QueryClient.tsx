'use client'

import {
  QueryClient,
  QueryClientProvider as TanstackQueryClientProvider
} from '@tanstack/react-query'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      refetchOnWindowFocus: false,
      refetchOnMount: false
    }
  }
})

export const QueryClientProvider = ({ children }: { children: React.ReactNode }) => {
  return <TanstackQueryClientProvider client={queryClient}>{children}</TanstackQueryClientProvider>
}
