'use client'

import { useMutation, useQueryClient } from '@tanstack/react-query'

import { openCase } from '../api/cases'

type UseOpenCaseProps = {
  onSuccess: ({ droppedLootItemId }: { droppedLootItemId: string }) => void
}

export const useOpenCase = ({ onSuccess }: UseOpenCaseProps) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: openCase,
    onSuccess: ({ data }) => {
      queryClient.invalidateQueries({
        queryKey: ['me']
      })
      const droppedLootItemId = data.result
      onSuccess({ droppedLootItemId })
    }
  })
}
