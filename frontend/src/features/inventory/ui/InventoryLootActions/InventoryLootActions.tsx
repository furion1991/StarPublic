'use client'

import { FormProvider, useForm } from 'react-hook-form'
import { useState } from 'react'

import { Button, ConfirmModal, Switch } from '@/shared/ui'

import classes from './InventoryLootActions.module.scss'

export const InventoryLootActions = () => {
  const [isSellConfirmModalOpen, setSellConfirmModalOpen] = useState(false)
  const useFormProps = useForm()

  const openSellConfirmModal = () => {
    setSellConfirmModalOpen(true)
  }

  return (
    <div className={classes.inventoryLootActions}>
      <div className={classes.canSellFilter}>
        <span>Можно продать</span>

        <FormProvider {...useFormProps}>
          <Switch name='canSellFilter' />
        </FormProvider>
      </div>

      <Button color='purple'>Вывести</Button>

      <Button onClick={openSellConfirmModal}>Продать все</Button>

      <ConfirmModal
        open={isSellConfirmModalOpen}
        text='Вы уверены что хотите продать все предметы?'
        onConfirm={() => {
          setSellConfirmModalOpen(false)
        }}
        onClose={() => {
          setSellConfirmModalOpen(false)
        }}
      />
    </div>
  )
}
