'use client'

import { useMutation, useQueryClient } from '@tanstack/react-query'

import { openCase } from '../api/cases'

import { Loot } from '@/entities/loot'

type DroppedLoot = Loot & {
  inventoryId: string
}

type UseOpenCaseProps = {
  onSuccess: ({ droppedLootItem }: { droppedLootItem: DroppedLoot }) => void
}

export const useOpenCase = ({ onSuccess }: UseOpenCaseProps) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: openCase,
    onSuccess: ({ data }) => {
      // for balance update
      queryClient.invalidateQueries({
        queryKey: ['me']
      })

      const { inventoryRecordId: inventoryId, item } = data.result.items[0]

      const droppedLootItem = {
        inventoryId,
        ...item
      }

      onSuccess({ droppedLootItem })
    }
  })
}
